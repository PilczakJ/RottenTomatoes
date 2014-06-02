$ cucumber
	Feature: See if movie is fresh
		Top box office movie must be fresh
		
		Scenario: top box office movie is fresh
			Given I am on the Index Page
			Then I scroll down
			Given I see the text "Top Box Office"
			Given that I don't see the text "Upcoming"
			Then I press list item number 1
			Then I wait for the MoviePage screen to appear
			Then I see the "textView2"
			Then the view with id "imageView2"'s tag property should equal "fresh"
			
		Scenario: top box office movie is rotten
			Given I am on the Index Page
			Then I scroll down
			Given I see the text "Top Box Office"
			Given that I don't see the text "Upcoming"
			Then I press list item number 1
			Then I wait for the MoviePage screen to appear
			Then I see the "textView2"
			Then the view with id "imageView2"'s tag property should equal "rotten"
			
