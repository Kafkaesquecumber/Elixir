#version 150 core

in vec2 _position;
in vec4 _color;
in vec2 _texCoord;

out vec4 color;
out vec2 texCoord;

uniform mat4 transform;

void main()
{
	color = _color;
	texCoord = _texCoord;
	gl_Position = transform * vec4(_position, 0.0, 1.0);
}