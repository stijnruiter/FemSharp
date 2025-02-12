#version 330 core
layout (location = 0) in vec3 vertPosition;
layout (location = 1) in vec4 vertColor;

out vec4 color;

void main()
{
    gl_Position = vec4(vertPosition, 1.0);
    color = vertColor;
}