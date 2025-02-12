#version 330 core

in float FemValue;
out vec4 FragColor;
uniform vec4 drawColor; 

void main()
{
    FragColor = drawColor;
}