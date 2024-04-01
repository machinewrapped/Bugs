import os
import logging
import ollama
import subprocess
import appdirs

### Test to generate content using the ollama server ###
### Launches ollama server in a subprocess, fetches available models, and generates content in a loop ###

### The second request always fails ###
### DEBUG - send_request_headers.started request=<Request [b'POST']>
### DEBUG - send_request_headers.complete
### DEBUG - send_request_body.started request=<Request [b'POST']>
### DEBUG - send_request_body.complete
### DEBUG - receive_response_headers.started request=<Request [b'POST']>
### ... hangs here, never receives response from server ###

test_content = [
    """
    This is a test. Please verify that you are able to generate content which forms a coherent response.
    """,

    """
    Please translate this text into English: 
    "Come Drink With Me est un film absolument fondamental dans l'histoire du cinéma hongkongais, 
    et plus spécifiquement dans l'histoire du cinéma d'arts martiaux, 
    avec un sous-genre spécifique dans le cinéma d'arts martiaux, 
    à savoir Wu Xia Pian, le film de chevalerie chinois." 
    """
]

logging.basicConfig(level=logging.DEBUG, format='%(asctime)s - %(levelname)s - %(message)s')

def _get_ollama_path():
    if os.getenv('OLLAMA_PATH'):
        return os.getenv('OLLAMA_PATH')

    if os.name == 'nt':
        install_path = appdirs.user_data_dir('Ollama', 'Programs')
        ollama_path = os.path.join(install_path, 'ollama.exe')
    else:
        ollama_path = '/usr/local/bin/ollama'

    return ollama_path

class OllamaTest:
    def __init__(self):
        self.ollama_path = _get_ollama_path()
        self.ollama_running = False
        self.ollama_process = None
        self.ollama_models = []

    @property
    def model_list(self):
        return ",".join(self.model_names)

    @property
    def model_names(self):
        return [model.get('name') for model in self.ollama_models]

    def CheckOllama(self):
        """ Check if the ollama binary exists and is executable """
        if not os.path.exists(self.ollama_path):
            logging.error(f"Ollama binary not found at {self.ollama_path}")
            return False

        if not os.access(self.ollama_path, os.X_OK):
            logging.error(f"Ollama binary is not executable at {self.ollama_path}")
            return False

        return True        

    def StartServer(self):
        """ Start an ollama server in a subprocess """
        try:
            self.ollama_process = subprocess.Popen([str(self.ollama_path), "serve"], stdout=subprocess.PIPE)
            self.ollama_running = True

        except Exception as e:
            logging.error(f"Unable to start ollama server: {e}")
            self.ollama_running = False
    
    def StopServer(self):
        """ Stop the ollama server if it is running and we started it """
        if self.ollama_running and self.ollama_process:
            self.ollama_process.terminate()
            self.ollama_running = False

    def FetchModels(self):
        """Fetch the list of available models from the ollama server"""
        try:
            if not self.ollama_running:
                self.StartServer()

            models = ollama.list()
            self.ollama_models = models.get('models', []) if models else []
            self.ollama_running = True

        except Exception as e:
            logging.info(f"Unable to retrieve ollama model list: {e}")
            self.ollama_running = False

    def GenerateContent(self, model_name : str, prompt : str):
        """ Generate content using a model """
        if not self.ollama_running:
            self.StartServer()

        content = ollama.generate(model_name, prompt)
        return content
    
def test_ollama():
    test = OllamaTest()
    assert test.CheckOllama() == True, "Ollama binary not found"

    test.StartServer()
    assert test.ollama_running == True, "Ollama server not running"

    test.FetchModels()
    assert len(test.ollama_models) > 0, "No ollama models available"

    logging.info(f"Available models: {test.model_list}")

    model_name = "openchat" if "openchat" in test.model_names else test.model_names[0]

    for content in test_content:
        response = test.GenerateContent(model_name, content)
        text = response.get('response', None)
        assert text is not None, "No response from ollama"

        logging.info(f"### Prompt ###\n{content}\n### Response ###\n {text}\n\n")

    test.StopServer()
    assert test.ollama_running == False, "Ollama server not stopped"

if __name__ == '__main__':
    try:
        test_ollama()

        logging.info("All tests passed!")

    except Exception as e:
        logging.error(f"Tests failed: {str(e)}")

