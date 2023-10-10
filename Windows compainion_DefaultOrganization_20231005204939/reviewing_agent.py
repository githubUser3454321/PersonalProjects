import openai
import re

class ReviewingAgent:
    def __init__(self, api_key):
        self.api_key = api_key
        openai.api_key = api_key

    def review_code(self, code, user_input):
        # Extend the prompt with context
        prompt = f"Hello, you are a large language model tasked with reviewing and correcting a CMD command. The command to be reviewed is: '<cmd>{code}</cmd>'. Make sure to place <cmd> tags befor and a </cmd> after wards. Most problems are in incorrect formating, by missing the last closing tag of '</cmd>'. also be sure that the command does what the user wanted, if that is not the case correct the command. user input:{user_input}, you may answer now. Be sure to write the closing'</cmd> tag after the command."
        # Use the OpenAI API to interact with the ChatGPT model
        response = openai.Completion.create(
            engine="text-davinci-003",
            prompt=prompt,
            max_tokens=50,  # Adjust the max_tokens as needed
            n=1,  # Number of responses to generate
            stop=None,  # You can specify a stopping criterion if needed
        )
        cmd_command = self.extract_cmd_command(response.choices[0].text)
        return cmd_command

    def extract_cmd_command(self, text):
        if (text in "</cmd>"):
            if(text not in "</cmd"):
                text = text.Replace("</cmd","</cmd>")
            else: 
                if(text not in "</cm"):
                    text = text.Replace("</cm","</cmd>")
                else:
                    if(text not in "</c"):
                        text = text.Replace("</c","</cmd>")
                    else:
                        if(text not in "</"):
                            text = text.Replace("</","</cmd>")
        cmd_pattern = r"<cmd>(.*?)<\/cmd>"
        match = re.search(cmd_pattern, text)
        print(f"output{text}")
        if match:
            cmd_command = match.group(1)
        else:
            cmd_command = "No cmd command found"
        return cmd_command


