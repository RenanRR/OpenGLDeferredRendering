#version 330
                                                                        
in vec2 TexCoord;                                                                  
in vec3 Nrm;                                                                    
in vec3 WorldPos;    

layout (location = 0) out vec3 WorldPosOut;   
layout (location = 1) out vec4 DiffuseOut;     
layout (location = 2) out vec3 NormalOut; 
layout (location = 3) out vec4 LightOut; 

uniform sampler2D Texture;              

uniform vec3 AmbientDirection;
uniform vec3 AmbientColor;
uniform float AmbientPower;  

void main()
{
    DiffuseOut      = vec4(texture2D(Texture, TexCoord).xyz,1);
    WorldPosOut     = WorldPos;
    NormalOut       = normalize(Nrm);

    float dt = 1.0 - (max(dot(NormalOut, -AmbientDirection),0.0) * 0.5);

    LightOut = vec4(AmbientColor * AmbientPower * dt, 1);
}

