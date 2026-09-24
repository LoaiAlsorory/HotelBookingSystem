import os
import time
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By

options = Options()
options.add_argument('--headless')
options.add_argument('--window-size=1366,768')
options.add_argument('--no-sandbox')
options.add_argument('--disable-dev-shm-usage')
options.add_argument('--disable-gpu')

out_dir = r'E:\Project_ASP.NET\HOTEL_BOOKIN_SYSTEM\Hotel_Booking_System\docs\screenshots'
os.makedirs(out_dir, exist_ok=True)

driver = webdriver.Chrome(options=options)
try:
    # 1. Login page
    print('Capturing Login page...')
    driver.get('http://localhost:5200/Account/Login')
    time.sleep(2)
    driver.save_screenshot(os.path.join(out_dir, 'web_01_login.png'))
    print('Saved web_01_login.png')

    # Perform login
    print('Submitting login...')
    try:
        email_el = driver.find_element(By.NAME, 'Email')
        pass_el = driver.find_element(By.NAME, 'Password')
        email_el.send_keys('admin@hotel.com')
        pass_el.send_keys('Password123!')
        driver.find_element(By.CSS_SELECTOR, "button[type='submit']").click()
        time.sleep(2)
    except Exception as e:
        print('Login form exception:', e)

    # 2. Main Dashboard
    print('Capturing Dashboard...')
    driver.get('http://localhost:5200/')
    time.sleep(2)
    driver.save_screenshot(os.path.join(out_dir, 'web_02_dashboard.png'))
    print('Saved web_02_dashboard.png')

    # 3. Hotels Page
    print('Capturing Hotels page...')
    driver.get('http://localhost:5200/Hotels')
    time.sleep(2)
    driver.save_screenshot(os.path.join(out_dir, 'web_03_hotels.png'))
    print('Saved web_03_hotels.png')

    # 4. Rooms Page
    print('Capturing Rooms page...')
    driver.get('http://localhost:5200/Rooms')
    time.sleep(2)
    driver.save_screenshot(os.path.join(out_dir, 'web_04_rooms.png'))
    print('Saved web_04_rooms.png')

    # 5. Bookings Page
    print('Capturing Bookings page...')
    driver.get('http://localhost:5200/Bookings')
    time.sleep(2)
    driver.save_screenshot(os.path.join(out_dir, 'web_05_bookings.png'))
    print('Saved web_05_bookings.png')

    # 6. Swagger API Documentation
    print('Capturing Swagger UI...')
    driver.set_window_size(1366, 950)
    driver.get('http://localhost:5242/swagger/index.html')
    time.sleep(3)
    driver.save_screenshot(os.path.join(out_dir, 'api_01_swagger.png'))
    print('Saved api_01_swagger.png')

finally:
    driver.quit()

print('All screenshots captured successfully!')
