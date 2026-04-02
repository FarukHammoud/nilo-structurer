import requests
from datetime import datetime

url = 'http://localhost:5067/pricer'

EUR = dict (
    name = "Euro",
    code = "EUR"
)

cashFlow = dict(
    kind='cash_flow',
    dates=[datetime.now().isoformat()],
    values=[100],
    currency=EUR
)

pricingDictionary = dict(
    indicators = ['premium'],
    contracts = [cashFlow]
)

resp = requests.post(url=url, json=pricingDictionary)
print(resp.content) # Check the JSON Response Content documentation below
