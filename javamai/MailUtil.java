package util;

import javax.mail.Message;
import javax.mail.Session;
import javax.mail.Transport;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;
import javax.mail.internet.MimeBodyPart;
import javax.mail.internet.MimeMultipart;
import java.util.Properties;

public class MailUtil {

    private static final String outlook_smtp = "smtp.office365.com";
    private static final String outlook_acc = "";
    private static final String outlook_pwd = "";

    private static final String qq_smtp = "smtp.qq.com";
    private static final String qq_acc = "";
    private static final String qq_pwd = "";

    private static final String icloud_smtp = "smtp.mail.me.com";
    private static final String icloud_acc = "";
    private static final String icloud_pwd = "";

    public static void sendMail(String subject, String content, String provider,
                String[] recpList, String[] copyList) throws Exception {

        if (recpList == null || recpList.length == 0) {
            Log.Error("NO RECIPIENTS!");
        }

        Properties props = new Properties();

        String smtp = null, account = null, pwd = null, sender = null;

        switch (provider) {
        case "qq":
            smtp = qq_smtp;
            account = qq_acc;
            pwd = qq_pwd;
            sender = account;
        break;
        case "icloud":
            smtp = icloud_smtp;
            account = icloud_acc;
            pwd = icloud_pwd;
            sender = "mail@wwr-blog.com";
        break;
        case "outlook":
        default:
            smtp = outlook_smtp;
            account = outlook_acc;
            pwd = outlook_pwd;
            sender = account;
        }

        props.setProperty("mail.host", smtp);
        props.setProperty("mail.transport.protocol", "smtp");
        props.setProperty("mail.smtp.port", "587");

        props.put("mail.smtp.auth", "true");
        props.put("mail.smtp.starttls.enable", "true");

        // Session session = Session.getDefaultInstance(props);
        Session session = Session.getInstance(props); // getDefaultInstance is singleton

        InternetAddress[] addrTo = new InternetAddress[recpList.length];
        for (int i = 0; i < addrTo.length; i++) {
            addrTo[i] = new InternetAddress(recpList[i]);
        }

        InternetAddress[] addrCC = null;
        if (copyList != null && copyList.length > 0) {
            addrCC = new InternetAddress[copyList.length];
            for (int i = 0; i < addrTo.length; i++) {
                addrTo[i] = new InternetAddress(copyList[i]);
            }
        }

        MimeMessage message = new MimeMessage(session);
        MimeMultipart multipart = new MimeMultipart();
        MimeBodyPart text = new MimeBodyPart();
        text.setContent(content, "text/html;charset=UTF-8");
        multipart.addBodyPart(text);

        message.setFrom(new InternetAddress(sender));
        // message.setHeader("header", "Good noon");
        message.setSubject(subject);
        message.setContent(multipart);

        message.setRecipients(MimeMessage.RecipientType.TO, addrTo);
        if (addrCC != null && addrCC.length != 0) {
            message.setRecipients(MimeMessage.RecipientType.CC, addrCC);
        }

        // message.setSentDate(new Date());

        Transport transport = session.getTransport();
        transport.connect(account, pwd);
        transport.sendMessage(message, message.getAllRecipients());
        Log.Info("succeed to send mail using " + account);

        transport.close();
    }
}
