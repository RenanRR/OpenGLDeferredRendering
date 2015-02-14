#version 330

out vec4 FragColor;       

in vec2 TexCoord;

uniform sampler2D ColorBuffer;
uniform sampler2D LightBuffer;

void main()
{
    FragColor = texture2D(ColorBuffer, TexCoord) * texture2D(LightBuffer, TexCoord);
}

