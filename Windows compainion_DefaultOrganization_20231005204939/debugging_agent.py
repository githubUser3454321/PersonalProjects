import openai
import re
from reviewing_agent import ReviewingAgent
class DebuggingAgent:
    def __init__(self, api_key):
        self.api_key = api_key
        openai.api_key = api_key

    def debug_command(self, code, errorMessage, original_code, user_input):
        # Extend the prompt with context
        prompt = f"Hello, you are a large language model tasked with debugging and correcting a CMD command. The command to be corrected is: <cmd>{code}</cmd>. Please provide feedback and any necessary corrections, and make sure to replace 'YOUR CORRECTED CMD COMMAND HERE' with the actual corrected CMD command within <cmd> tags. you need to answer with a cmd command. The Error Message that we got is:'{errorMessage}'. Additional information:'Userinput:{user_input}','original code executed:{original_code}'."
        # Use the OpenAI API to interact with the ChatGPT model
        response = openai.Completion.create(
            engine="text-davinci-003",
            prompt=prompt,
            max_tokens=50,  # Adjust the max_tokens as needed
            n=1,  # Number of responses to generate
            stop=None,  # You can specify a stopping criterion if needed
        )
        cmd_command = self.extract_cmd_command(response.choices[0].text)


        reviewing_agent = ReviewingAgent('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')
        review_result = reviewing_agent.review_code(cmd_command, user_input)
        print(f"Reviewed CMD Command: {review_result}")

        return cmd_command

    def extract_cmd_command(self, text):
        cmd_pattern = r"<cmd>(.*?)<\/cmd>"
        match = re.search(cmd_pattern, text)
        print(f"output{text}")
        if match:
            cmd_command = match.group(1)
        else:
            cmd_command = "No cmd command found"
        return cmd_command