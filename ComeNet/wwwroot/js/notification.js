var time = new Date();
var formattedDateTime = time.toLocaleString();

console.log(formattedDateTime);
var memberId = null;


document.addEventListener('click', function (event) {
	if (event.target.id === 'videochat') {

		var h4Tags = document.querySelectorAll('a');
		h4Tags.forEach(function (h4Tag) {
			h4Tag.addEventListener('click', function () {
				memberId = h4Tag.getAttribute('data-userid');
				console.log('Member ID:', memberId);
			});
		});

		/////發送邀請api
		var myHeaders = new Headers();
		myHeaders.append("Content-Type", "application/json");
		var rawvideo = JSON.stringify({
			"articleHeading": formattedDateTime + "聊天室邀請",
			"articleContent": "https://johnny666.online/chatroom/19",
			"userId": memberId,
		});
		var requestvideo = {
			method: 'POST',
			headers: myHeaders,
			body: rawvideo,
			redirect: 'follow'
		};

		fetch("https://johnny666.online/api/users/SendToSpecificUser", requestvideo)
			.then(response => response.json())
			.then(result => {
				if (result.userId != null)
				{
					console.log(result.userId);					
				}
				else {
					alert('請稍等');					
				}
			})
			.catch(error => console.log('error', error));
	}
	if (event.target.id === 'chat')
	{
		window.open('https://johnny666.online/Home/chat2user', '_blank');
	}
});