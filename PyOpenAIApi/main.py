from openai import OpenAI
import json

f = open("api_key.txt", "r")

client = OpenAI(
    api_key = f.read()
)

example_json = '''{
  "contents": [
    {
      "name": String,
      "difficulty": Number
    }
  ]
}'''

prompt = '''Provide valid JSON output. 
Provide a list of the main study contents for my college exam and their difficulty.
Provide the number of contents based in the main theme of my exam and the number of days until the exam.
Difficulty should be a number between 1 an 10, where 1 is very easy and 10 is very difficult.
Provide a list called 'content' with the an object with the properties 'name' and 'difficult'.
The main theme of my exam is: World war I and World war II.
The number of days until the exam is: 7.'''

chat_completion = client.chat.completions.create(
    model="gpt-3.5-turbo-0125",
    response_format={"type":"json_object"},
    messages=[
        {"role":"system","content":"Provide output in valid JSON. The data schema should follow this schema: "+json.dumps(example_json)},
        {"role":"user","content":prompt}
    ]
)

finish_reason = chat_completion.choices[0].finish_reason

if(finish_reason == "stop"):
    data = chat_completion.choices[0].message.content
    contents = json.loads(data)['contents']
    
    print(f"Contents length: {len(contents)}\n")
    
    for content in contents:
        print("Name: " + content['name'] + ", Difficulty: " + str(content['difficulty']))
else :
    print("Error! provide more tokens please")