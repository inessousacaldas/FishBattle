/*
* @Author: cilu2
* @Date:   2015-12-31 15:36:52
* @Last Modified by:   anchen
* @Last Modified time: 2016-06-29 10:28:02
*/



#if defined (__cplusplus)
extern "C" {
#endif
    
    extern char * __getValueFromInfoPlist(char *keyPath);
    extern void __copyToClipboard(char *value);
#if defined (__cplusplus)
}
#endif


#if defined (__cplusplus)
extern "C"
{
#endif

#pragma mark 
    char * __getValueFromInfoPlist(char *keyPath)
    {
        if (keyPath == nil)
        {
            return "";
        }
        
        NSString *path = [[NSBundle mainBundle] pathForResource:@"Info" ofType:@"plist"];
        NSLog(@"%@", path);
        
        NSString *value = @"";
        NSDictionary *plistData = [NSDictionary dictionaryWithContentsOfFile:path];
        NSObject *tValue = [plistData valueForKeyPath:[NSString stringWithUTF8String:keyPath]];
        if (tValue == nil || ![tValue isKindOfClass:[NSString class]])
        {
            // do nothing
        }
        else
        {
            value = (NSString*)tValue;
        }
        
        const char *charValue = [value UTF8String];
        char *str = (char *)malloc(strlen(charValue) + 1);
        strcpy(str, charValue);
        
        return str;
    }
    
    void __copyToClipboard(char *value)
    {
        NSString *str = nil;
        if (value == nil)
        {
            str = @"";
        }
        else
        {
            str = [NSString stringWithUTF8String:value];
        }
        UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
        pasteboard.string = str;
    }

#if defined (__cplusplus)
}
#endif