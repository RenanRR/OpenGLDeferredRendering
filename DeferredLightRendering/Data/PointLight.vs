#version 330                                                                        
in vec3 Position;                                                                                       

uniform mat4 View;     
uniform mat4 Proj;     
uniform mat4 World;      

void main()
{       
    gl_Position  = Proj * View * World * vec4(Position, 1.0);
}