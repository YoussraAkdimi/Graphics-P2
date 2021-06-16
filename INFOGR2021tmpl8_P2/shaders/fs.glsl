#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
uniform sampler2D pixels;	// texture sampler

// shader output
out vec4 outputColor;

uniform vec3 lightPos;
uniform vec3 lightColor = vec3(1,1,1);

// fragment shader
void main()
{
    //ambient
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    vec3 result = ambient * normal.xyz;
    outputColor = texture( pixels, uv ) + vec4(result, 1);
}