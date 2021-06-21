#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
in vec3 fragPos;

uniform sampler2D pixels;	// texture sampler

// shader output
out vec4 outputColor;

uniform vec3 lightPos = vec3(50, 50, -100);
uniform vec3 lightColor = vec3(0.5, 0.5, 0.5);
uniform vec3 viewPos = vec3(0, 0, -15);
uniform vec3 objectColor;

// fragment shader
void main()
{
    //ambient
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    //diffuse
    vec4 lightDir = vec4(lightPos, 1) - vec4(fragPos, 1);
    vec4 norm = normalize(normal);
    vec4 lightDirN = normalize(lightDir);
    float diff = dot(norm, lightDirN);
    diff = max(diff, 0.0);
    vec3 diffuse = diff * lightColor;

    //specular
    float specularStrength = 0.000000001;
    vec4 viewDir = normalize(vec4(viewPos,1) - vec4(fragPos,1));
    vec4 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 8);
    vec3 specular = specularStrength * spec * lightColor;


    //combining shaders
    vec3 result = (ambient + diffuse + specular);
    outputColor = texture( pixels, uv ) + vec4(result, 1);
}