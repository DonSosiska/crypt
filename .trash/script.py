import gnupg
import os
















gpg = gnupg.GPG(gnupghome="C:\\Users\\ladmael\\Documents\\gpg")

with open('./firestarter firestarter@syneforge.com (0xDBE05045DF931C93) pub.asc', 'r') as f:
    key_data = f.read()
imported_keys = gpg.import_keys(key_data)
print(imported_keys)
plaintext = "Hello, World!"

ciphertext = str(gpg.encrypt(plaintext, imported_keys.fingerprints[0], always_trust=True))
os.system(f'echo "{ciphertext}" | mail -s "Subject" ladmael@syneforge.com')
import smtplib
from email.mime.text import MIMEText
msg = MIMEText(ciphertext)
msg['Subject'] = 'Subject'
msg['From'] = 'ladmael@syneforge.com'
msg['To'] = 'ladmael@syneforge.com'

with smtplib.SMTP('mail.syneforge.com', 587) as server:
    server.starttls()
    server.login('ladmael@syneforge.com', '2@')
    server.sendmail('ladmael@syneforge.com', 'firestarter@syneforge.com', msg.as_string())
