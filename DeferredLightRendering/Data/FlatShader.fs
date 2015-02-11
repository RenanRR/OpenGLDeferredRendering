#version 330
                                                                        
in vec2 TexCoord;                                                                  

uniform sampler2D Texture;                

out vec4 FragColor;

void main()
{
    FragColor = texture2D(Texture, TexCoord);
    if(FragColor.a < 0.2)
        discard;
}

