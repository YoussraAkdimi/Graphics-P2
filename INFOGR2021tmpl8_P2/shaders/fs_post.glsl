#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform vec2 uResolution;

// shader output
out vec3 outputColor;

//calculates Colors with vignette implemented
vec3 GetVignette(vec3 color)
{
	vec2 position = uv - vec2(0.5);
	float dist = length(position);
	float radius = 1.0;
	float strength = 0.02;
	float vignette = smoothstep(radius, strength, dist);
	color.rgb = color.rgb - (1.0 - vignette);

	return color;
}

void main()
{
	//chromatic aberration
	float p1 = texture(pixels, uv).r;
	float p2 = texture(pixels, uv + vec2(0.01, 0)).g;
	float p3 = texture(pixels, uv + vec2(0.01, 0)).b;
	vec3 abbColor = vec3(p1, p2, p3);

	//combining vignette and aberration colors
	vec3 vigColor = GetVignette(abbColor);

	outputColor = vigColor;

	//if post processing wants to be turned off
	//outputColor = texture(pixels, uv).rgb;
}

// EOF