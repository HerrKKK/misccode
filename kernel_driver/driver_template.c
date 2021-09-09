    // SPDX-License-Identifier: GPL-2.0-only
    /*
     *  A Simple Driver Template
     *  Author: Wang Weiran <wang.wei.ran@outlook.com>
     */
    #include <linux/platform_device.h>
    #include <linux/module.h>
    #include <linux/fs.h>
    #define TRUE 1
    #define DEVICE_NAME "wwr"
    #define CLASS_NAME "wwr_class"
    #define CLASS_NODE TRUE
    #define DEVICE_NODE TRUE
    typedef struct wwr_device {
        void* buf;
        int buf_len;
    } wwr_device_t;
    static int wwr_open(struct inode *inode, struct file *filp)
    {
        pr_info("%s\n", __func__);
        return 0;
    }
    static int wwr_release(struct inode *inode, struct file *filp)
    {
        pr_info("%s\n", __func__);
        return 0;
    }
    static ssize_t wwr_read(struct file *filp, char *buff,
                size_t count, loff_t *offp)
    {
        size_t len = 0;
        pr_info("%s\n", __func__);
        return len;
    }
    static ssize_t wwr_write(struct file *filp, const char *buff,
                size_t count, loff_t *offp)
    {
        size_t len = 0;
        pr_info("%s\n", __func__);
        return len;
    }
    static struct file_operations wwr_fops = {
        .owner = THIS_MODULE,
        .open = wwr_open,
        .release = wwr_release,
        .read = wwr_read,
        .write = wwr_write,
    };
    #if CLASS_NODE
    static ssize_t wwr_class_node_show(struct class *class,
                    struct class_attribute *attr, char *buf) {
        pr_info("%s\n", __func__);
        return sprintf(buf, "%s\n", __func__);
    }
    static ssize_t wwr_class_node_store(struct class *class,
             struct class_attribute *attr, const char *buf, size_t count) {
        pr_info("%s\n", __func__);
        return count;
    }
    #endif
    /*
     CLASS_ATTR under /sys/class/CLASS_NAME, DRIVER_ATTR under /sys/devices,
     BUS_ATTR under /sys/bus while DRIVER_ATTR under /sys/drivers
     wwr is a prefix which is applies to show/store func
     and 'class_create_file' and the name for the node
    */
    #if CLASS_NODE
    static CLASS_ATTR_RW(wwr_class_node);
    #endif
    #if DEVICE_NODE
    static ssize_t wwr_dev_node_show(struct device *dev,
                        struct device_attribute *attr, char *buf)
    {
        pr_info("%s\n", __func__);
        return sprintf(buf, "%s\n", __func__);
    }
    static ssize_t wwr_dev_node_store(struct device *dev,
                        struct device_attribute *attr,
                        const char *buf, size_t len)
    {
        pr_info("%s\n", __func__);
        return len;
    }
    static DEVICE_ATTR(wwr_dev_node, 0664,
            wwr_dev_node_show, wwr_dev_node_store);
    static DEVICE_ATTR(wwr_dev_node2, 0664,
            wwr_dev_node_show, wwr_dev_node_store);
    #endif
    struct class* wwr_class;
    static int major;
    static struct wwr_device wwr_dev = {
        .buf = NULL,
        .buf_len = 0,
    };
    static int wwr_probe(struct platform_device *pdev)
    {
        int ret;
        struct device* dev;
        pr_info("%s\n", __func__);
        ret = 0;
        wwr_dev.buf_len = 0;
        // register a device, major as 0 to automatically alloc one
        major = register_chrdev(0, DEVICE_NAME, &wwr_fops);
        if (major < 0) {
            pr_err("failed to register a char device");
            return major;
        }
        // create the /sys/class/CLASS_NAME
        wwr_class = class_create(THIS_MODULE, CLASS_NAME);
        if (IS_ERR(wwr_class)) {
            pr_err("failed to create class\n");
            return -1;
        }
    #if CLASS_NODE
        // create /sys/class/CLASS_NAME/wwr_class_node
        ret = class_create_file(wwr_class, &class_attr_wwr_class_node);
        if (ret) {
            pr_err("failed to create class nodes\n");
            class_destroy(wwr_class);
            unregister_chrdev(major, DEVICE_NAME);
            return ret;
        }
    #endif
        /*
         create /dev/DEVICE_NAME AND /sys/class/DEVICE_NAME
         which means, the device node under /sys/class
         is related with that under /dev
         and the struct device* it returns points at /sys/class/DEVICE_NAME
        */
        dev = device_create(wwr_class, NULL, MKDEV(major, 0), NULL, DEVICE_NAME);
        if (dev == NULL) {
            pr_err("failed to create device");
            class_destroy(wwr_class);
            unregister_chrdev(major, DEVICE_NAME);
            return -1;
        }
    #if DEVICE_NODE
        // create a node under device(under DEVICE_NAME this case)
        ret = device_create_file(dev, &dev_attr_wwr_dev_node);
        if (ret) {
            pr_err("failed to create device node\n");
            device_destroy(wwr_class, MKDEV(major, 0));
            class_destroy(wwr_class);
            unregister_chrdev(major, DEVICE_NAME);
            return ret;
        }
        // the same as above
        ret = sysfs_create_file(&(dev->kobj), &dev_attr_wwr_dev_node2.attr);
        if (ret) {
            pr_err("failed to create device node\n");
            device_destroy(wwr_class, MKDEV(major, 0));
            class_destroy(wwr_class);
            unregister_chrdev(major, DEVICE_NAME);
        }
    #endif
        // sysfs_create_file(struct kobject*, device_attribute*);
        // can be used to create device node recursively
        return ret;
    }
    static int wwr_remove(struct platform_device *pdev)
    {
        pr_info("%s\n", __func__);
        device_destroy(wwr_class, MKDEV(major, 0));
        class_destroy(wwr_class);
        unregister_chrdev(major, DEVICE_NAME);
        return 0;
    }
    static void wwr_dev_release(struct device * dev)
    {
        pr_info("%s\n", __func__);
    }
    static struct platform_device wwr_platform_device = {
        .name = DEVICE_NAME,
        .id = -1, // -1 represents there is only 1 device
        /* we must provide a release function or it would crash
         when the kernel tried to uninstall the module.*/
        .dev = {
            .release = wwr_dev_release,
        }
    };
    static struct platform_driver wwr_platform_driver = {
        .probe = wwr_probe,
        .remove = wwr_remove,
        .suspend = NULL,
        .resume = NULL,
        .driver = {
            .name = DEVICE_NAME,
            .owner = THIS_MODULE
        },
    };
    static int __init wwr_driver_init(void)
    {
        int ret = 0;
        pr_info("%s\n", __func__);
        ret = platform_device_register(&wwr_platform_device);
        ret &= platform_driver_register(&wwr_platform_driver);
        return ret;
    }
    static void __exit wwr_driver_exit(void)
    {
        pr_info("%s\n", __func__);
        platform_driver_unregister(&wwr_platform_driver);
        platform_device_unregister(&wwr_platform_device);
    }
    module_init(wwr_driver_init);
    module_exit(wwr_driver_exit);
    // module_platform_driver(wwr_platform_driver);
    MODULE_AUTHOR("Wang Weiran <wang.wei.ran@outlook.com>");
    MODULE_DESCRIPTION("A platform template");
    MODULE_LICENSE("GPL");
