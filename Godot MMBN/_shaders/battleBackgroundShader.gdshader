shader_type canvas_item;

uniform bool _scaleXOnly;
uniform float _scale;
uniform float _frames;
uniform float _scrollSpeed;

varying vec2 _extraUV;

void vertex(){
	// holy shit this is jank
	if (VERTEX_ID == 0)
	{
		_extraUV = vec2(0, 0);
	}
	else if (VERTEX_ID == 3)
	{
		_extraUV = vec2(1, 0);
	}
	else if (VERTEX_ID == 1)
	{
		_extraUV = vec2(0, 1);
	}
	else if (VERTEX_ID == 2)
	{
		_extraUV = vec2(1, 1);
	}
}

void fragment() {
	vec2 modifiedUV = _extraUV * _scale;
	if (_scaleXOnly)
		modifiedUV.y /= _scale;
	
	modifiedUV -= TIME * _scrollSpeed;
	modifiedUV = fract(modifiedUV);
	
	float uvWidth = 1.0f / _frames;
	float frame = floor(UV.x / uvWidth);
	
	modifiedUV.x = mix(frame * uvWidth, (frame + 1.0f) * uvWidth, modifiedUV.x);
	
	COLOR = texture(TEXTURE, modifiedUV);
	
	//COLOR = vec4(frame/_frames, 0, 0, 1);
	//COLOR = vec4(modifiedUV, 0, 1);
}
