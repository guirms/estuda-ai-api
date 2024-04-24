from openai import OpenAI
import json

f = open("api_key.txt", "r")

client = OpenAI(
    api_key = f.read()
)

example_json = {
  "ski_resorts": [
    {
      "name": "Les Portes du Soleil",
      "slope_kilometers": 600
    }
  ]
}

prompt = "Provide valid JSON output. Provide the top 10 largest ski resorst in Europe. Rankin them on slope kilometers (descending) Provide one column 'name' and a column 'slope_kilometers' representing the total slope kilometers"

chat_completion = client.chat.completions.create(
    model="gpt-3.5-turbo-1106",
    response_format={"type":"json_object"},
    messages=[
        {"role":"system","content":"Provide output in valid JSON. The data schema should be like this: "+json.dumps(example_json)},
        {"role":"user","content":prompt}
    ]
)

finish_reason = chat_completion.choices[0].finish_reason

if(finish_reason == "stop"):
    data = chat_completion.choices[0].message.content

    ski_resorts = json.loads(data)

    for ski_resort in ski_resorts['ski_resorts']:
        print(ski_resort['name']+" : "+str(ski_resort['slope_kilometers'])+"km")
else :
    print("Error! provide more tokens please")