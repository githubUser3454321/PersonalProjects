import subprocess
from reviewing_agent import ReviewingAgent
from debugging_agent import DebuggingAgent
from chatgpt_agent import ChatGPTAgent
def main():
    while True:
        user_input = input("Enter your command: ")
        # Get cmd command from ChatGPT model
        while True:
            chatgpt_agent = ChatGPTAgent('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')
            cmd_command = chatgpt_agent.get_cmd_command(user_input)
            print(f"Original CMD Command: {cmd_command}")
    
            if cmd_command != "":
                while True:
                    reviewing_agent = ReviewingAgent('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')
                    review_result = reviewing_agent.review_code(cmd_command, user_input)
                    if (review_result != "No cmd command found"):
                        break
                print(f"Reviewed CMD Command: {review_result}")    
                break
        user_choice = input("Do you want to execute? (yes/no): ")
        if user_choice.lower() == "no":
            continue
        error_message, has_error = execute_command(review_result)
        while has_error != "":
            debugging_agent = DebuggingAgent('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')
            user_choice = input("Do you want to execute? (yes/no): ")
            if user_choice.lower() == "yes" or "y":
                error_message, has_error = execute_command( debugging_agent.debug_command(review_result, error_message, review_result, user_input) )
            else: 
                break
        user_choice = input("Do you want to continue? (yes/no): ")
        if user_choice.lower() != "yes":
            break
def execute_command(cmd_command, run_as_admin=True):
    cmd_command = f"cmd.exe /c {cmd_command}"
    try:
        result = subprocess.run(cmd_command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, shell=True)
    except Exception as e:
        return str(e),True
    
    print(result)
    if result.returncode != 0:
        return result.stderr.strip(),False  # Return the error message
    else:
        return result.stdout.strip(),False 
    '''        
def execute_command(cmd_command, run_as_admin=True):
    if run_as_admin:
        # If running as admin, create a runas command
        cmd_command = f"cmd.exe /c {cmd_command}"
        runas_command = ["runas", "/user:Administrator"] + cmd_command.split()
        try:
            # Use shell=False for better compatibility and security
            result = subprocess.run(runas_command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, shell=True)
        except Exception as e:
            return str(e)  # Return any exceptions raised
    else:
        try:
            # Use shell=False for better compatibility and security
            result = subprocess.run(cmd_command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, shell=True)
        except Exception as e:
            return str(e)  # Return any exceptions raised
        '''
if __name__ == "__main__":
    main()