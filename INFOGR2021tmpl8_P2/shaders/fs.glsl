﻿#version 330
 
// shader input
in vec2 uv;			// interpolated texture coordinates
in vec4 normal;			// interpolated normal
in vec3 fragPos;

uniform sampler2D pixels;	// texture sampler

// shader output
out vec4 outputColor;

uniform vec3 lightPos = vec3(50, 50, -50);
uniform vec3 lightColor = vec3(1,1,1);
uniform vec3 objectColor;

// fragment shader
void main()
{
    //ambient
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    //diffuse
    vec4 lightDir = vec4(lightPos,1) - vec4(fragPos, 1);
    vec4 norm = normalize(normal);
    vec4 lightDirN = normalize(lightDir);
    float diff = dot(norm, lightDirN);
    diff = max(diff, 0.0);
    vec3 diffuse = diff * lightColor;

    //combining shaders
    vec3 result = (ambient + diffuse) * normal.xyz;
    outputColor = texture( pixels, uv ) + vec4(result,1);
}