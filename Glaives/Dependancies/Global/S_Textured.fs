#version 150 core
in vec4 color;
in vec2 texCoord;
out vec4 finalColor;

uniform sampler2D textureSampler;

void main() 
{ 
	finalColor = texture(textureSampler, texCoord.xy) * color;
}