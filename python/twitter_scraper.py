import sys
import time
import pandas as pd

from datetime import datetime
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.firefox.options import Options
from selenium.webdriver.common.proxy import *
from selenium.webdriver.common.by import By

start_time = datetime.now()
options = webdriver.FirefoxOptions()
options.set_preference('network.proxy.type', 1)
options.set_preference('network.proxy.http', '127.0.0.1')
options.set_preference('network.proxy.http_port', 8081)
options.set_preference('network.proxy.ssl', '127.0.0.1')
options.set_preference('network.proxy.ssl_port', 8081)

options.add_argument('--headless')
options.add_argument('--disable-gpu')

driver = webdriver.Firefox(options=options)

url = 'https://twitter.com/'
id = 'JoeBiden'

try:
    id = sys.argv[1]
except:
    pass
print('scrap on', url + id + '/with_replies')
 
driver.get(url + id + '/with_replies')
time.sleep(10)

scroll_height, tweet_array, tweet_dup = 0, [], {}
count_list = ['comments','retweets','favorites']

try:
    while True:

        driver.execute_script('window.scrollBy(0,1000)')
        time.sleep(3)

        sp = BeautifulSoup(driver.page_source, 'lxml')

        divs = sp.find_all('article',
                            {'class': 'css-1dbjc4n r-1loqt21 r-18u37iz r-1ny4l3l r-1udh08x r-1qhn6m8 r-i023vh r-o7ynqc r-6416eg',
                             'role': 'article',
                             'data-testid': 'tweet'
                            })

        for div in divs:
            data_dict = {'date':'',
                         'content':'',
                         'mention':'',
                         'hashtag':'',
                         'comments':0,
                         'retweets':0,
                         'favorites':0,
                         'isRetweet':0
                        }

            try:
                # parse time
                tweet_date = div.find('time')
                data_dict['date'] = tweet_date['datetime'].replace('T', ' ').replace('-', '/')[:-8]

                # parse comments, retweets and favorites
                count_divs = div.find_all('span',
                            {'data-testid': 'app-text-transition-container'})

                for i, count_div in enumerate(count_divs):
                    count = count_div.find('span', {'class':'css-901oao css-16my406 r-poiln3 r-bcqeeo r-qvutc0'})
                    if count != None:
                        data_dict[count_list[i]] = count.get_text()
            except:
                print('exception at comments', data_dict['date'])

            # parse content
            try:
                content_div = div.find('div', 
                    {'class': 'css-901oao r-18jsvk2 r-37j5jr \
                     r-a023e6 r-16dba41 r-rjixqe r-bcqeeo r-bnwqim r-qvutc0'})
                content = content_div.find('span', {'class': 'css-901oao \
                    css-16my406 r-poiln3 r-bcqeeo r-qvutc0'}).get_text()
            except:
                content = ''
                print('no content tweet ' + data_dict['date'])

            # check if duplicated
            if content in tweet_dup.keys() and tweet_dup[content] == tweet_date['datetime']:
                # print('there is a duplicated data at', tweet_date)
                continue
            tweet_dup[content] = tweet_date['datetime']
            data_dict['content'] = content

            # check if there is a hashtags or ats
            try:
                link_divs = div.find_all('a', {'role': 'link',
                    'class': 'css-4rbku5 css-18t94o4 css-901oao css-16my406 r-1cvl2hr r-1loqt21 r-poiln3 r-bcqeeo r-qvutc0'})

                for i, link_div in enumerate(link_divs):
                    split, mention, hashtag = '', data_dict['mention'], data_dict['hashtag']

                    if i != 0:
                        split = ','

                    if link_div.get_text()[0] == '@':
                        data_dict['mention'] += split + link_div.get_text()
                    elif link_div.get_text()[0] == '#':
                        data_dict['hashtag'] += split + link_div.get_text()
            except:
                print("exception at hashtag " + data_dict['date'])

            # check if it is a retweet
            try:
                isRetweet = div.find('span',
                        {'class': 'css-901oao css-16my406 css-cens5h r-14j79pv r-poiln3 r-n6v787 r-b88u0q r-1cwl3u0 r-bcqeeo r-qvutc0',
                        'data-testid': 'socialContext'})
                if isRetweet.get_text()[-9:] == "Retweeted":
                    data_dict['isRetweet'] = 1
            except:
                data_dict['isRetweet'] = 0

            tweet_array.append(data_dict)

        # check the end
        height = driver.execute_script("return document.documentElement.scrollTop;")
        if scroll_height == height:
            print('reach the bottom at', tweet_array[-1]['date'], len(tweet_array), 'parsed')
            break
        else:
            scroll_height = height

except Exception as e:
    print(e)
finally:
    pf = pd.DataFrame(list(tweet_array))
    file_path = pd.ExcelWriter(id + '_tweets.xlsx')
    pf.to_excel(file_path, encoding='utf-8', index=True)
    file_path.save()
    print('Elapsed time:', (datetime.now()-start_time).seconds, 'seconds')
    driver.quit()