#version 330 core
layout (location = 0) in vec3 vertPosition;
layout (location = 1) in vec4 vertColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec4 color;

void main()
{
    gl_Position = vec4(vertPosition, 1.0) * model * view * projection;
    color = vertColor;
}