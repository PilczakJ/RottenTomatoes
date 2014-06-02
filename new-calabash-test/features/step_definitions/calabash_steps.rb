require 'calabash-android/calabash_steps'
require 'utilities.rb'

Given (/^I am on the Index Page$/) do
	wait_for(180) {element_exists("text id:'opening'")}
end

Then (/^I should see the view with property "([^\"]*)" equaling "([^\"]*)"$/) do |property,text|
	element_exists("Android.Widget.ImageView",property,text)
end