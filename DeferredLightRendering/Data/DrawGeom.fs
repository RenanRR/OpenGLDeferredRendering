#version 330
                                                                        
in vec2 TexCoord;                                                                  
in vec3 Nrm;                                                                    
in vec3 WorldPos;    

layout (location = 0) out vec3 WorldPosOut;   
layout (location = 1) out vec4 DiffuseOut;     
layout (location = 2) out vec3 NormalOut; 
						
uniform sampler2D Texture;              
											
void main()									
{											
	DiffuseOut      = vec4(texture2D(Texture, TexCoord).xyz,1);
	WorldPosOut     = WorldPos;					
	NormalOut       = normalize(Nrm);							
}

