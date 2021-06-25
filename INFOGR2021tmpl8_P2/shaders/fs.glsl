#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
in vec3 fragPos;

uniform sampler2D pixels;	// texture sampler

// shader output
out vec4 outputColor;

uniform bool lightOn = true;
uniform vec3 lightPos = vec3(250, 0, 250);
uniform vec3 lightColor = vec3(0.5, 0.5, 0.5);
uniform vec3 lightPos1 = vec3(-100, 0, 50);
uniform vec3 lightColor1 = vec3(0.5, 0.5, 0.5);
uniform vec3 viewPos = vec3(-100, 0, -50);          //the negative value of the cameraposition hardcoded
uniform vec3 objectColor;

vec3 PhongShader(vec3 pos, vec3 color, float ambientS, float specS)
{
    //diffuse
    vec4 lightDir = vec4(fragPos, 1) - vec4(pos, 1);
    vec4 norm = normalize(normal);
    vec4 lightDirN = normalize(lightDir);
    float diff = dot(norm, lightDirN);
    diff = max(diff, 0.0);

    //specular
    vec4 viewDir = normalize(vec4(viewPos,1) - vec4(fragPos,1));
    vec4 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 1);

    //combining shaders
    vec3 result = (ambientS + diff + (spec * specS)) * color;
    return result;
}

// fragment shader
void main()
{
    //different lightsources
    vec3 result = PhongShader(lightPos, lightColor, 0.1, 0.0025);
    vec3 result1 = PhongShader(lightPos1, lightColor1, 0.1, 0.00001);

    if(lightOn == true)
    {
        outputColor = texture( pixels, uv) + vec4(result, 1) + vec4(result1, 1);
    }
    if(lightOn == false) 
    {
        outputColor = texture( pixels, uv) + vec4(result, 1);
    }
}