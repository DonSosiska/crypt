import gnupg
import os
import smtplib
from email.mime.text import MIMEText

def send_encrypted_message(gpg, sender_email, sender_password, recipient_email, message):
    ciphertext = str(gpg.encrypt(message, recipient_email, always_trust=True))

    msg = MIMEText(ciphertext)
    msg['Subject'] = 'Subject'
    msg['From'] = sender_email
    msg['To'] = recipient_email

    try:
        with smtplib.SMTP('mail.syneforge.com', 587) as server:
            server.starttls()
            server.login(sender_email, sender_password)
            server.sendmail(sender_email, recipient_email, msg.as_string())
    except: 
        print("Отправлено")
    print(recipient_email)



gpg = gnupg.GPG(gnupghome="C:\\Users\\ladmael\\Documents\\gpg")

imported_keys = gpg.import_keys_file('./syneforge.pgp')
plaintext = "Hello, World!"
i=0
print(imported_keys.results)
for key in imported_keys.results:
    fingerprint = key['fingerprint']
    email = gpg.list_keys()[i]["uids"][0]
    i=i + 1
    send_encrypted_message(gpg, 'ladmael@syneforge.com', 'pass', email, plaintext)
print()
gpg.delete_keys(gpg.list_keys(True))
os.remove(os.path.join(gpg.gnupghome, "pubring.kbx"))
os.remove(os.path.join(gpg.gnupghome, "trustdb.gpg"))
print(gpg.list_keys())

