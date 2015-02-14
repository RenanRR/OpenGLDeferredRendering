#version 330

out vec4 FragColor;       

in vec2 TexCoord;

uniform sampler2D ColorBuffer;
uniform sampler2D LightBuffer;

uniform vec3 AmbientColor;
uniform float AmbientPower;  

void main()
{
    vec4 color = texture2D(ColorBuffer, TexCoord);

    vec4 light = texture2D(LightBuffer, TexCoord) + vec4(AmbientColor * AmbientPower,1);
    if(color.a == 0.0)
        discard;

    FragColor = color * light;
}

