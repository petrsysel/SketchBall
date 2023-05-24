shader_type canvas_item;

uniform vec4 color : hint_color;

void fragment(){
	vec4 previous = texture(TEXTURE, UV);
	previous.rgb = 1.0 - previous.rgb;
	previous.rgb *= color.rgb;
	COLOR = previous;
}