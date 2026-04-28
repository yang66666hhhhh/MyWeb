import sys, urllib.request, urllib.error
urls = ['http://localhost:5062/swagger/v1/swagger.json', 'http://localhost:7015/swagger/v1/swagger.json']
tried = []
for url in urls:
    tried.append(url)
    try:
        req = urllib.request.Request(url, headers={'User-Agent':'curl/7.0'})
        with urllib.request.urlopen(req, timeout=5) as r:
            status = r.getcode()
            body = r.read(2000)
            try:
                text = body.decode('utf-8', errors='replace')
            except:
                text = str(body)
        print('---COMMAND---')
        print('curl -v', url)
        print('---URL---')
        print(url)
        print('---STATUS---')
        print(status)
        print('---BODY-800---')
        print(text[:800])
        sys.exit(0)
    except Exception as e:
        print('---COMMAND---')
        print('curl -v', url)
        print('---URL---')
        print(url)
        print('---ERROR---')
        print(str(e))
print('No reachable URL. Tried:', ', '.join(tried))
