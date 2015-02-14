#version 330                                                                        
in vec3 Position;                                                                                       
in vec2 Tex;

out vec2 TexCoord;

void main()
{       
	TexCoord = Tex;

    gl_Position  = vec4(Position, 1.0);
}