INCLUDE ../GlobalSettings/global.ink
	# speaker: Гонец
	# portrait: ms_yellow_neutral
	# layout: right
	Пан, рылся в летописях и нашёл завещание вашего деда. 
	Говорят, он тайно спрятал фамильный клад в главном зале — да оставил загадку вместо карты.
	
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
    И что за загадка?
	# speaker: Гонец
	# portrait: ms_yellow_neutral
	# layout: right
    Всего одна строка, выведенная чернилами и кровью: «Где спины томов глядят в камень — там сердце золота». Больше ни слова.  
    
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
	*   Займусь поисками.
	    -> askDetails

	*   Старая байка.
	    -> dismissClaim

	=== askDetails ===
	    Надеюсь эта байка выведет вас к золоту… или к очередной легенде. Удачи.
	    # speaker: Гонец
	    # portrait: ms_yellow_neutral
	    # layout: right
	    ~ TalkedWithMagnate = true
	    -> END

	=== dismissClaim ===
    	Дед любил приукрасить. Архивы врут чаще, чем священник в трактире. Не буду тратить время на сказки.
	    # speaker: Гонец
	    # portrait: ms_yellow_neutral
	    # layout: right
        Как пожелаете. Но чашу с золотом редко наполняет тот, кто не ищет.
        ~ QuestComplite = true
	    -> END