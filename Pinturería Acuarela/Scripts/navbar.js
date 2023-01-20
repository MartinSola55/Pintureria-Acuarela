// --------------add active class-on another-page move----------
jQuery(document).ready(function ($) {
	// Get current path and find target link
	var path = window.location.pathname.split("/").pop();

	// Account for home page with empty path
	if (path == '') {
		path = 'Home';
	}

	var target = $('#navbarContainer ul li a[href="/' + path + '"]');
	// Add active class to target link
	target.parent().addClass('active');
}); 


$(".navbar-toggler").click(function () {
	$(".navbar-collapse").slideToggle(300);
});