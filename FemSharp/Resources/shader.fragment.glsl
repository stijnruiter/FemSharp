#version 330 core

in float FemValue;
out vec4 FragColor;
uniform vec4 drawColor; 

void main()
{
    if (drawColor.w > 0.0f)
    {
        FragColor = drawColor;
    }
    else
    {
        FragColor = vec4(FemValue, 0.1f, 0.1f, 1.0f);
    }
}