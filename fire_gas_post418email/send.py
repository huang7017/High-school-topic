import serial
import time
import requests
import smtplib
ser = serial.Serial('/dev/ttyACM0', baudrate = 9600,timeout=0)
device_id = "Ds07057l"
device_key = "8qEKhns1kUHJT20m"
TO = 'email'
GMAIL_USER = 'email'
GMAIL_PASS = 'password'
SUBJECT = '居家安全警告'
TEXT = '已發生危險請盡快回家'

def send_email():
    print("Sending Email")
    smtpserver = smtplib.SMTP("smtp.gmail.com",587)
	 # smtpserver = smtplib.SMTP("so-net.net.tw")
    smtpserver.ehlo()
    smtpserver.starttls()
    smtpserver.ehlo
    smtpserver.login(GMAIL_USER, GMAIL_PASS)
    header = 'To:' + TO + '\n' + 'From: ' + GMAIL_USER
    header = header + '\n' + 'Subject:' + SUBJECT + '\n'
    print header
    msg = header + '\n' + TEXT + ' \n\n'
    smtpserver.sendmail(GMAIL_USER, TO, msg)
    smtpserver.close()
def MCS_upload(dchn,value):
        url_up = "http://api.mediatek.com/mcs/v2/devices/" + device_id
        url_up += "/datapoints.csv"
        data = dchn+",,"+str(value)
        r = requests.post(url_up,headers = {"deviceKey" : device_key,'Content-Type':'text/csv'},data=data)
        print r.text
a= ""
print "Start"
while True:

        ser.write('a\r')
        x = ser.inWaiting()

        if x>0:
                for i in range (x):
                        a += ser.read()
                print a

                sensor = a.split(',')
                MCS_upload("gas",sensor[0])
                MCS_upload("sun",sensor[1])
				MCS_upload("FIRE",sensor[2])
				#if sensor[0]+'' == 'Gas leak':
					#send_email()
				#if sensor[2]+'' == 'ON Fire':
					#send_email()
				MCS_upload("uv",sensor[3])
        a =  ""
        time.sleep(1)
