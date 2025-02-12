#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in float gradientValue;

out float gradient;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    gradient = gradientValue;
}