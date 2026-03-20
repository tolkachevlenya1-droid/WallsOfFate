INCLUDE ../GlobalSettings/global.ink
	# speaker: Плотник
	# portrait: black_man
	# layout: right
    П-пощады, пан! Я продался за 30 серебреников, думал — бумага да и только…
    
    # speaker: Плотник
    # portrait: drunk_carpenter
    # layout: left 
	*   Монеты заплатят кровью.
	    -> executeTraitor

	*  Исчезни — и молись.
	    -> exileTraitor

	=== executeTraitor ===
	    # speaker: Плотник
	    # portrait: black_man
	    # layout: right
	    Хотел лишь семье хлеба добыть… 
	    
	    # speaker: Плотник
	    # portrait: black_man
	    # layout: left
	    Семья утешится твоими тридцатью монетами.
	    ~ PeopleSatisfaction -= 1
	     ~ Gold += 30
	    ~ CastleStrength += 60
	    -> END

	=== exileTraitor ===
	    # speaker: Плотник
	    # portrait: black_man
	    # layout: left
	    Один раз соскользнёшь за порог — голова с плеч. Стой жди стражу.
	    # speaker: Плотник
	    # portrait: black_man
	    # layout: right
	    Только не трогайте мою семью...
	    ~ Gold += 30
	    ~ CastleStrength += 20
	    ~ PeopleSatisfaction += 1
	    -> END