#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in float femValue;

out float FemValue;
void main()
{
    gl_Position = vec4(aPosition, 1.0);
    FemValue = femValue;
}