import gnupg
import os
import smtplib
import getpass
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email.mime.image import MIMEImage

def send_encrypted_message(gpg, recipient_email, message, subject, sender_email, sender_password):
    # создаем сообщение с несколькими частями
    msg = MIMEMultipart()
    msg['Subject'] = subject
    msg['From'] = sender_email
    msg['To'] = recipient_email

    # добавляем текстовую часть сообщения
    text = MIMEText(message, 'plain', 'utf-8')
    msg.attach(text)

    # добавляем изображения
    with open('1.jpg', 'rb') as f:
        img = MIMEImage(f.read())
        msg.attach(img)

    # шифруем сообщение
    ciphertext = str(gpg.encrypt(msg.as_bytes(), recipient_email.encode('utf-8'), always_trust=True))

    # создаем MIME-часть для зашифрованного сообщения
    ciphertext_part = MIMEText(ciphertext, 'plain', 'utf-8')
    ciphertext_part.set_charset('utf-8')
    ciphertext_part.replace_header('Content-Disposition', 'inline')
    ciphertext_part.replace_header('Content-Transfer-Encoding', 'quoted-printable')
    msg.attach(ciphertext_part)

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

for key in imported_keys.results:
    fingerprint = key['fingerprint']
    email = gpg.list_keys()[i]["uids"][0]
    i=i + 1
    send_encrypted_message(gpg, email, plaintext, subject, sender_email, sender_password)

print()
gpg.delete_keys(gpg.list_keys(True))
os.remove(os.path.join(gpg.gnupghome, "pubring.kbx"))
os.remove(os.path.join(gpg.gnupghome, "trustdb.gpg"))
print(gpg.list_keys())
