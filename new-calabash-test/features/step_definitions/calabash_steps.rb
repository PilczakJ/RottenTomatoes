require 'calabash-android/calabash_steps'
require 'utilities.rb'

Given (/^I am on the Index Page$/) do
	wait_for(180) {element_exists("text id:'opening'")}
end