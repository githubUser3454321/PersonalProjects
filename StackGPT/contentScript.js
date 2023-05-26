(() => {
    let chatAnswerElement;
    chrome.runtime.onMessage.addListener((obj, sender, response) => {
    if(status == false) return;
        const { type, value, videoId } = obj;
        if (type === "NEW")
            LoadNewChatGptAnswer();
    });


    const LoadNewChatGptAnswer = () => {
        getstatus(function (status) {
        var on = status;
        console.log("StackGPT is set to: "+on);
        if(on== false) return;
        
        const questionTitleElement = document.querySelector(".question-hyperlink");
        if (!questionTitleElement) return;
        const questionexpandetElement = document.getElementsByClassName("s-prose js-post-body");
        if (!questionexpandetElement) return;
        chatAnswerElement = document.getElementById("chat-answer-section");
        if (chatAnswerElement) return;

        var question = questionTitleElement.textContent;
        var questionInformation = questionexpandetElement[0].textContent;

        chatAnswerElement = document.createElement("div");
        chatAnswerElement.id = "chat-answer-section";
        
  

        getAnswerFromGPT(question, questionInformation)
            .then((reply) => {
                chatAnswerElement.innerHTML = `
            <hr style="background-color:#FFD700 class="user">Matthias Oberholzer:</hr>
<body>
  <div class="answer-block">
    <pre><p>`+ escapeHtml(reply) + `</p></pre>
  </div>
</body>
`;
                console.log(reply);
                questionTitleElement.parentNode.insertBefore(chatAnswerElement, questionTitleElement.nextSibling);
            })
            .catch((error) => {console.error('Error:', error);});
            });
    };
    function escapeHtml(string) {
        var element = document.createElement('div');
        element.innerText = string;
        return element.innerHTML;
    }
    function savestatus(boolToSave) {
        chrome.storage.local.set({ "Status": boolToSave }, function () {});
    }
    function getstatus(callback) {
        chrome.storage.local.get("Status", function (result) {
            callback(result.Status);
        });
    }

function getAnswerFromGPT(question, questionInformation) {
  return new Promise((resolve, reject) => {
    chrome.storage.local.get("KEY", function (result) { 
      const apiKey = result.KEY;
      const apiUrl = 'https://api.openai.com/v1/chat/completions';
      const payload = {
          model: 'gpt-3.5-turbo',
          messages: [
              { role: 'system', content: 'You are ChatGPT, a large language model trained by OpenAI.' },
              { role: 'user', content: `title: ${question}` },
              { role: 'assistant', content: `body: ${questionInformation}` },
          ],
      };
      const headers = {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${apiKey}`,
      };
        console.log(apiKey);
      fetch(apiUrl, {
          method: 'POST',
          headers: headers,
          body: JSON.stringify(payload),
      })
      .then((response) => response.json())
      .then((data) => {
          const choices = data.choices;
          if (!choices || choices.length === 0) throw new Error('No choices found in the response');
          const reply = choices[0].message.content;
          if (!reply) throw new Error('No content found in the message');
          resolve(reply);
      })
      .catch((error) => {
          console.error('Error:', error);
          reject(error);
      });
    });
  });
}
    LoadNewChatGptAnswer();
})();