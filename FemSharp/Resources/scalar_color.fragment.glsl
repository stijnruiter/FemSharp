#version 330 core

in float gradient;

out vec4 FragColor;

void main()
{
    FragColor = vec4(gradient, 0.6f, 0.3f, 1.0f);
}