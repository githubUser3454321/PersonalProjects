import openai
import re

class ChatGPTAgent:
    def __init__(self, api_key):
        self.api_key = api_key
        openai.api_key = api_key

    def get_cmd_command(self, user_input):
        # Extend the prompt with context
        prompt = f"Hello you are a large language model needed to generate a cmd command from the input. the users want to UserInput:[{user_input}] your answer should contain one opening and closing CMD tag<cmd>YOUR CMD COMMAND HERE</cmd> not that you need to replace the 'YOUR CMD COMMAND HERE' with a actual cmd command."
        # Use the OpenAI API to interact with the ChatGPT model
        response = openai.Completion.create(
            engine="text-davinci-003",
            prompt=prompt,
            max_tokens=50,  # Adjust the max_tokens as needed
            n=1,  # Number of responses to generate
            stop=None,  # You can specify a stopping criterion if needed
        )
        return self.extract_cmd_command(response.choices[0].text)

    def extract_cmd_command(self, text):
        # Extract the cmd command enclosed in <cmd> tags using regex
        if len(text) == 0: 
            return ""
        cmd_pattern = r"<cmd>(.*?)<\/cmd>"
        match = re.search(cmd_pattern, text)
        print(f"output{text}")
        
        if match:
            cmd_command = match.group(1)
        else : 
            if match is not None and match.group(1) is not None:
                cmd_command = f"No cmd command found: the command may be contained in this message : {match.group(1)}"
            else:
                cmd_command = f"No cmd command found: the command may be contained in this message : {text}"
        
        return cmd_command
