#version 330                                                                        
in vec3 Position;                                                                                       
in vec2 Tex;

uniform mat4 View;     
uniform mat4 Proj;     
uniform mat4 World;      

out vec2 TexCoord;

void main()
{       
	TexCoord = Tex;

    gl_Position  = Proj * View * World * vec4(Position, 1.0);
}