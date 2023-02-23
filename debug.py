
import gnupg
import os
import smtplib
import getpass
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email.mime.image import MIMEImage

def send_encrypted_message(gpg, recipient_email, message, subject, sender_email, sender_password):
    # шифруем сообщение
    ciphertext = str(gpg.encrypt(message.encode('utf-8'), recipient_email.encode('utf-8'), always_trust=True))

    # создаем сообщение с зашифрованным текстом
    msg = MIMEMultipart()
    msg['Subject'] = subject
    msg['From'] = sender_email
    msg['To'] = recipient_email
    msg.attach(MIMEText(ciphertext))
    
    img = MIMEImage(open('1.jpg', 'rb').read())
    img.add_header('Content-Disposition', 'attachment', filename='1.jpg.asc')
    msg.attach(img)


    try:
        with smtplib.SMTP('mail.syneforge.com', 587) as server:
            server.starttls()
            server.login(sender_email, sender_password)
            server.sendmail(sender_email, recipient_email, msg.as_bytes())
            print("Отправлено " + recipient_email)
    except: 
        print("Не отправлено " + recipient_email)



gpg = gnupg.GPG(gnupghome="./gpg")

imported_keys = gpg.import_keys_file('./syneforge.pgp')

f = open('message.txt', encoding="utf-8")
plaintext = f.read()

subject = input("Введите тему сообщения: ")
sender_email = input("Введите свою почту (email@syneforge.com): ")
sender_password = getpass.getpass("Введите свой пароль от почты: ")

id = 0

for key in imported_keys.results:
    fingerprint = key['fingerprint']
    email = gpg.list_keys()[id]["uids"][0]
    id = id + 1
    send_encrypted_message(gpg, email, plaintext, subject, sender_email, sender_password)

print()
gpg.delete_keys(gpg.list_keys(True))
os.remove(os.path.join(gpg.gnupghome, "pubring.kbx"))
os.remove(os.path.join(gpg.gnupghome, "trustdb.gpg"))
print(gpg.list_keys())
