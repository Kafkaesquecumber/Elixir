// MIT License
// 
// Copyright(c) 2018 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using Elixir.Input;
using Elixir.Internal;
using Elixir.Internal.Input;

namespace Elixir.GameFramework
{
    /// <summary>
    /// Base class for any object that is part of a level hierarchy and can be transformed
    /// </summary>
    public class Actor 
    {
        /// <summary>
        /// The unique name of the actor
        /// </summary>
        public UniqueString Name { get; set; }
    
        /// <summary>
        /// Enumerates the children for this actor (no sub-children)
        /// </summary>
        public IEnumerable<Actor> Children => _children.ToArray(); // ToArray to allow for re-parenting during iteration

        private Actor _parent;
        /// <summary>
        /// Parent the actor to another actor (cannot be null, use Unparent to parent the actor to the level root)
        /// </summary>
        public Actor Parent
        {
            get => _parent;
            set
            {
                if(Immutable)
                {
                    return;
                }

                if(value == null && !PendingDestruction)
                {
                    throw new ElixirException("Actor must have a parent, call Unparent to parent the actor to the level root actor");
                }

                if(value == this || value == _parent)
                {
                    return;
                }
                
                if (_parent != null)
                {
                    // Parent my children to my parent if my new parent is my current child
                    bool newParentIsCurrentChild = false;
                    DoRecursive(x =>
                    {
                        if (x == value)
                        {
                            newParentIsCurrentChild = true;
                        }
                    }, false);

                    if (newParentIsCurrentChild)
                    {
                        for (int i = _children.Count - 1; i >= 0; i--)
                        {
                            _children[i].SetParentInternal(_parent);
                        }
                    }
                }

                if (value != null)
                {
                    SetParentInternal(value);
                }
                else
                {
                    _parent._children.Remove(this);
                    _parent = null;
                }
            }
        }

        /// <summary>
        /// This is only part of the Parent.Set code, Do not call this function from outside the Parent.Set
        /// </summary>
        /// <param name="value"></param>
        private void SetParentInternal(Actor value)
        {
            value.TryUpdateMatrices();

            if (_parent != null)
            {
                #region Temporarely convert to local space

                _localPosition = _parent.WorldMatrix.TransformPoint(_localPosition);

                _localRotation += _parent._rotation;
                _localRotation = Utils.MathEx.Clamp360(_localRotation);

                Matrix tempScalMat = Matrix.Identity;
                tempScalMat.Scale(_parent._scale);
                _localScale = tempScalMat.TransformPoint(_localScale);

                _position = _localPosition;
                _rotation = _localRotation;
                _scale = _localScale;

                #endregion

                Actor oldParent = _parent;
                oldParent._children.Remove(this);
            }
            value._children.Add(this);

            _localPosition = value.InverseWorldMatrix.TransformPoint(_localPosition);

            _localRotation -= value._rotation;
            _localRotation = Utils.MathEx.Clamp360(_localRotation);

            Matrix scalMat = Matrix.Identity;
            scalMat.Scale(value._scale);
            _localScale = scalMat.GetInverse().TransformPoint(_localScale);

            _position = _localPosition;
            _rotation = _localRotation;
            _scale = _localScale;

            _parent = value;

            WorldMatrixIsDirty = true;
            InverseWorldMatrixIsDirty = true;
            TryUpdateMatrices();
        }

        private Vector2 _localPosition;
        /// <summary>
        /// The position in local space
        /// </summary>
        public Vector2 LocalPosition
        {
            get
            {
                TryUpdateMatrices();
                if (Parent != null)
                {
                    return _localPosition - LocalAbsoluteOrigin;
                }

                return _localPosition;
            }
            set
            {
                if (Parent != null)
                {
                    Vector2 vec = _parent.WorldMatrix.TransformPoint(value + LocalAbsoluteOrigin);
                    Position = vec;
                }
                else
                {
                    Position = value;
                }
            }
        }

        private float _localRotation;
        /// <summary>
        /// The rotation in local space
        /// </summary>
        public float LocalRotation
        {
            get
            {
                TryUpdateMatrices();
                return _localRotation;
            }
            set
            {
                if (Parent != null)
                {
                    Rotation = value + _parent._rotation;
                }
                else
                {
                    Rotation = value;
                }
            }
        }

        private Vector2 _localScale;
        /// <summary>
        /// 
        /// </summary>
        public Vector2 LocalScale
        {
            get
            {
                TryUpdateMatrices();
                return _localScale;
            }
            set
            {
                if (Parent != null)
                {
                    Matrix scalMat = Matrix.Identity;
                    scalMat.Scale(_parent._scale);
                    Scale = scalMat.TransformPoint(value);
                }
                else
                {
                    Scale = value;
                }
            }
        }

        private Vector2 _position;
        /// <summary>
        /// The position in world space
        /// </summary>
        public Vector2 Position
        {
            get
            {
                TryUpdateMatrices();
                return _position;
            }
            set
            {
                if(Parent != null)
                {
                    Vector2 result = _parent.InverseWorldMatrix.TransformPoint(value);
                    if(_localPosition != result)
                    {
                        _localPosition = result;
                        WorldMatrixIsDirty = true;
                        InverseWorldMatrixIsDirty = true;
                    }
                }
                else
                {
                    if(_localPosition != value)
                    {
                        _localPosition = value;
                        WorldMatrixIsDirty = true;
                        InverseWorldMatrixIsDirty = true;
                    }
                }

                _position = value;
            }
        }

        private float _rotation;
        /// <summary>
        /// The rotation in world space
        /// </summary>
        public float Rotation
        {
            get
            {
                TryUpdateMatrices();
                return _rotation;
            }
            set
            {
                float clampedValue = Utils.MathEx.Clamp360(value);

                if(Parent != null)
                {
                    float result = clampedValue - _parent._rotation;
                    if(_localRotation != result)
                    {
                        _localRotation = result;
                        WorldMatrixIsDirty = true;
                        InverseWorldMatrixIsDirty = true;
                    }
                }
                else
                {
                    if(_localRotation != clampedValue)
                    {
                        _localRotation = clampedValue;
                        WorldMatrixIsDirty = true;
                        InverseWorldMatrixIsDirty = true;
                    }
                }

                _rotation = clampedValue;
            }
        }

        private Vector2 _scale;
        /// <summary>
        /// The scale in world space
        /// </summary>
        public Vector2 Scale
        {
            get
            {
                TryUpdateMatrices();
                return _scale;
            }
            set
            {
                if(Parent != null)
                {
                    Matrix scalMat = Matrix.Identity;
                    scalMat.Scale(_parent._scale);
                    Vector2 result = scalMat.GetInverse().TransformPoint(value);
                    if(_localScale != result)
                    {
                        _localScale = result;
                        WorldMatrixIsDirty = true;
                        InverseWorldMatrixIsDirty = true;
                    }
                }
                else
                {
                    if(_localScale != value)
                    {
                        _localScale = value;
                        WorldMatrixIsDirty = true;
                        InverseWorldMatrixIsDirty = true;
                    }
                }

                _scale = value;
            }
        }
        
        private Vector2 _up;
        /// <summary>
        /// The up vector
        /// </summary>
        public Vector2 Up
        {
            get
            {
                TryUpdateMatrices();
                return _up;
            }
        }

        private Vector2 _right;
        /// <summary>
        /// The right vector
        /// </summary>
        public Vector2 Right
        {
            get
            {
                TryUpdateMatrices();
                return _right;
            }
        }

        /// <summary>
        /// The local space matrix
        /// </summary>
        public Matrix LocalMatrix { get; private set; }

        private Matrix _worldMatrix;

        /// <summary>
        /// The world space matrix
        /// </summary>
        public Matrix WorldMatrix
        {
            get
            {
                if (WorldMatrixIsDirty)
                {
                    ForceUpdateWorldMatrix();
                }

                return _worldMatrix;
            }
        }

        private Matrix _inverseWorldMatrix;
        /// <summary>
        /// The inverse world space matrix
        /// </summary>
        public Matrix InverseWorldMatrix
        {
            get
            {
                if (InverseWorldMatrixIsDirty)
                {
                    ForceUpdateInverseWorldMatrix();
                }

                return _inverseWorldMatrix;
            }
        }

        private bool _inputEnabled;
        /// <summary>
        /// Wheter or not the actor should receive input events
        /// </summary>
        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                if (_inputEnabled != value)
                {
                    _inputEnabled = value;
                    if (value)
                    {
                        Engine.Get.Window.Interface.InputActionEvent += ReceiveInputAction;
                        Engine.Get.Window.Interface.InputAxisEvent += ReceiveInputAxis;
                    }
                    else
                    {
                        Engine.Get.Window.Interface.InputActionEvent -= ReceiveInputAction;
                        Engine.Get.Window.Interface.InputAxisEvent -= ReceiveInputAxis;
                    }
                }
            }
        }
        
        /// <summary>
        /// A shorthand to the engine singleton
        /// </summary>
        protected Engine Engine => Engine.Get;
        
        /// <summary>
        /// <para>Contains functionality for binding input to function handlers</para>
        /// <para>Each actor has it's own input binder</para>
        /// </summary>
        protected InputBinder InputBinder { get; } = new InputBinder();

        protected internal virtual FloatRect LocalBoundsInternal { get; } = FloatRect.Zero;
        protected internal virtual Vector2 OriginInternal { get; } = Vector2.Zero;

        internal Vector2 LocalAbsoluteOrigin => new Vector2(LocalBoundsInternal.Width * OriginInternal.X, LocalBoundsInternal.Height * OriginInternal.Y);
        private readonly List<Actor> _children = new List<Actor>();

        internal bool PendingDestruction { get; private set; }

        /// <summary>
        /// Whether or not the actor can be re-parented or destroyed (internal use only)
        /// </summary>
        internal readonly bool Immutable;

        protected internal bool WorldMatrixIsDirty;
        protected internal bool InverseWorldMatrixIsDirty;

        /// <inheritdoc />
        public Actor()
            : this(false)
        {
        }

        internal Actor(bool immutable)
        {
            Immutable = immutable;
            Name = new UniqueString(GetType().Name);

            _scale = Vector2.Unit;
            _localScale = Vector2.Unit;
            _up = new Vector2(0, 1);
            _right = new Vector2(1, 0);
            
            _worldMatrix = Matrix.Identity;

            WorldMatrixIsDirty = true;
            InverseWorldMatrixIsDirty = true;
            LocalMatrix = Matrix.Identity;

            InputEnabled = false;
            Engine.Get.OnActorConstruction(this);
        }
        
        /// <summary>
        /// Performs an action recursively for actors under this actor in the hierarchy (and self if includeSelf = true)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="includeSelf">Wheter or not we should perform this action on ourself as well</param>
        /// <returns>The amount of actors the action was operated on</returns>
        public int DoRecursive(Action<Actor> action, bool includeSelf = true)
        {
            int count = 0;
            DoRecursiveInternal(action, this, includeSelf, ref count);
            return count;
        }

        /// <summary>
        /// Parent the actor to the level root
        /// </summary>
        public void Unparent()
        {
            Parent = Engine.Get.LevelManager.Level.Root;
        }

        /// <summary>
        /// <para>Marks this actor for destroy and it's children recursively</para>
        /// <para>Marked actors will be destroyed at the end of the frame</para>
        /// </summary>
        public void Destroy()
        {
            if (Immutable)
            {
                Engine.Get.Debug.Warning($"Can not destroy immutable actor '{Name.String}'");
                return;
            }
            DoRecursive(x => x.DestroySingle());
        }

        /// <summary>
        /// Destroy even when immutable (used to unload levels)
        /// </summary>
        internal void ForceDestroy()
        {
            DoRecursive(x => x.DestroySingle());
        }

        public override string ToString()
        {
            return Name.String;
        }

        internal void InitializeRecursive()
        {
            DoRecursive(x => x.Initialize());
        }

        internal void TickInternal(float deltaTime)
        {
            if (InputEnabled)
            {
                foreach (KeyValuePair<string, InputBinder.InputActionEvent> inputActionEvent in InputBinder.InputActionEvents)
                {
                    foreach (RawInputAction rawInputAction in Engine.Get.InputManager.GetRawActionsByBindingId(inputActionEvent.Key))
                    {
                        inputActionEvent.Value(rawInputAction.KeyState, rawInputAction.Key, rawInputAction.GamepadId);   
                    }
                }

                foreach (KeyValuePair<string, InputBinder.InputAxisEvent> inputAxisEvent in InputBinder.InputAxisEvents)
                {
                    foreach (RawInputAxis rawInputAxis in Engine.Get.InputManager.GetRawAxesByBindingId(inputAxisEvent.Key))
                    {
                        //TODO: (untested) Test with a gamepad
                        inputAxisEvent.Value(rawInputAxis.Axis, rawInputAxis.Value, rawInputAxis.GamepadId);
                    }       
                }
            }
            Tick(deltaTime);
        }

        internal void ReceiveInputInternal(KeyState keyState, Key key, int gamepadId)
        {
            ReceiveInputAction(keyState, key, gamepadId);
        }

        private void ForceUpdateWorldMatrix()
        {
            float cosine = Utils.MathEx.FastCos(Utils.MathEx.ToRadians(_localRotation));
            float sine = Utils.MathEx.FastSin(Utils.MathEx.ToRadians(_localRotation));
            float sxc = _localScale.X * cosine;
            float syc = _localScale.Y * cosine;
            float sxs = _localScale.X * sine;
            float sys = _localScale.Y * sine;
            float tx = -LocalAbsoluteOrigin.X * sxc - LocalAbsoluteOrigin.Y * sys + _localPosition.X;
            float ty = LocalAbsoluteOrigin.X * sxs - LocalAbsoluteOrigin.Y * syc + _localPosition.Y;

            LocalMatrix = new Matrix(
                sxc, sys, tx,
                -sxs, syc, ty,
                0.0f, 0.0f, 1.0f
                );

            if (Parent != null)
            {
                Matrix parMat = _parent._worldMatrix;
                _worldMatrix = parMat * LocalMatrix;

                _position = _parent._worldMatrix.TransformPoint(_localPosition);
                _rotation = Utils.MathEx.Clamp360(_parent._rotation + _localRotation);
                Matrix scalMat = Matrix.Identity;
                scalMat.Scale(_parent._scale);
                _scale = scalMat.TransformPoint(_localScale);
            }
            else
            {
                _position = _localPosition;
                _worldMatrix = LocalMatrix;
            }

            DoRecursive(x =>
            {
                x.WorldMatrixIsDirty = true;
                x.InverseWorldMatrixIsDirty = true;
            }, false);

            float rot = Utils.MathEx.ToRadians(_rotation);
            float upRot = rot - Utils.MathEx.ToRadians(90.0f);
            
            _up.X = Utils.MathEx.FastCos(upRot);
            _up.Y = -Utils.MathEx.FastSin(upRot);
            _right.X = Utils.MathEx.FastCos(rot);
            _right.Y = -Utils.MathEx.FastSin(rot);

            WorldMatrixIsDirty = false;
        }

        private void ForceUpdateInverseWorldMatrix()
        {
            _inverseWorldMatrix = WorldMatrix.GetInverse();
            InverseWorldMatrixIsDirty = false;
        }

        internal void TryUpdateMatrices()
        {
            if (WorldMatrixIsDirty)
            {
                ForceUpdateWorldMatrix();
            }

            if (InverseWorldMatrixIsDirty)
            {
                ForceUpdateInverseWorldMatrix();
            }
        }

        private void DoRecursiveInternal(Action<Actor> action, Actor actor, bool includingSelf, ref int count, int depth = 0)
        {
            if(depth == 0)
            {
                if(includingSelf)
                {
                    action.Invoke(actor);
                    count++;
                }
            }
            else
            {
                action.Invoke(actor);
                count++;
            }
            
            foreach (Actor child in Children)
            {
                child.DoRecursiveInternal(action, child, includingSelf, ref count, depth + 1);
            }
        }

        private void CreateFlatHierarchy(ref List<Actor> flatHierarchy, Actor parent, bool includingSelf, int depth = 0)
        {
            if (depth == 0)
            {
                if (includingSelf)
                {
                    flatHierarchy.Add(parent);
                }
            }
            else
            {
                flatHierarchy.Add(parent);
            }
            
            foreach (Actor child in parent.Children)
            {
                CreateFlatHierarchy(ref flatHierarchy, child, includingSelf, depth + 1);
            }
        }
        
        private void DestroySingle()
        {
            if (PendingDestruction)
            {
                return;
            }

            Engine.CurrentLevel.PendingDestroyActors.Add(this);
            PendingDestruction = true;
        }
        
        /// <summary>
        /// Initialization happens after the whole level is loaded or immediately if the actor is created after level loading (base call not needed)
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// Called every tick
        /// </summary>
        /// <param name="deltaTime">The elapsed time in seconds since the last tick</param>
        protected virtual void Tick(float deltaTime) { }

        /// <summary>
        /// Process an input event if input is enabled for this actor
        /// </summary>
        /// <param name="keyState"></param>
        /// <param name="key">The key that triggered this event</param>
        /// <param name="gamepadId">The id of the gamepad that triggered this event (if it is triggered by a gamepad)</param>
        protected virtual void ReceiveInputAction(KeyState keyState, Key key, int gamepadId) { }

        /// <summary>
        /// Process an input axis event if input is enabled for this actor
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <param name="value">The value of the axis (can be between 0 and 1 or -1 and 1 depending on the type of axis)</param>
        /// <param name="gamepadId">The id of the gamepad that triggered this event (if it is triggered by a gamepad)</param>
        protected virtual void ReceiveInputAxis(InputAxis axis, float value, int gamepadId) { }
    }
}
