#if !TARGET_OS_TV


#import "CropController.h"

#import <Foundation/Foundation.h>
#import <MobileCoreServices/MobileCoreServices.h>
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#if UNITY_VERSION < 500
#import "iPhone_OrientationSupport.h"
#else
#import "OrientationSupport.h"
#endif

#ifndef NSFoundationVersionNumber_iOS_7_1
#define NSFoundationVersionNumber_iOS_7_1 1047.25
#endif

#define BELOW_IOS_8 (NSFoundationVersionNumber <= NSFoundationVersionNumber_iOS_7_1)


@interface UIImage (fixOrientation)

- (UIImage *)fixOrientation;

@end

@implementation UIImage (fixOrientation)

- (UIImage *)fixOrientation {
    
    // No-op if the orientation is already correct
    if (self.imageOrientation == UIImageOrientationUp) return self;
    
    // We need to calculate the proper transformation to make the image upright.
    // We do it in 2 steps: Rotate if Left/Right/Down, and then flip if Mirrored.
    CGAffineTransform transform = CGAffineTransformIdentity;
    
    switch (self.imageOrientation) {
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, self.size.height);
            transform = CGAffineTransformRotate(transform, M_PI);
            break;
            
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, 0);
            transform = CGAffineTransformRotate(transform, M_PI_2);
            break;
            
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, 0, self.size.height);
            transform = CGAffineTransformRotate(transform, -M_PI_2);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationUpMirrored:
            break;
    }
    
    switch (self.imageOrientation) {
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.width, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
            
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, self.size.height, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationDown:
        case UIImageOrientationLeft:
        case UIImageOrientationRight:
            break;
    }
    
    // Now we draw the underlying CGImage into a new context, applying the transform
    // calculated above.
    CGContextRef ctx = CGBitmapContextCreate(NULL, self.size.width, self.size.height,
                                             CGImageGetBitsPerComponent(self.CGImage), 0,
                                             CGImageGetColorSpace(self.CGImage),
                                             CGImageGetBitmapInfo(self.CGImage));
    CGContextConcatCTM(ctx, transform);
    switch (self.imageOrientation) {
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            // Grr...
            CGContextDrawImage(ctx, CGRectMake(0,0,self.size.height,self.size.width), self.CGImage);
            break;
            
        default:
            CGContextDrawImage(ctx, CGRectMake(0,0,self.size.width,self.size.height), self.CGImage);
            break;
    }
    
    // And now we just create a new UIImage from the drawing context
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
    return img;
}

@end


@interface IOSImagePickerManager : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate, PassImageDelegate>

{
    CGSize _cropSize;
    BOOL _allowsEditing;
}

+ (id)   sharedInstance;
- (void) saveToCameraRoll:(NSString*)media;
- (void) GetVideoPathFromAlbum;
-(void) PickImage:(int) source AllowsEditing:(bool)allowsEditing CropSize:(CGSize)size;

@end


@implementation IOSImagePickerManager

static IOSImagePickerManager * _sharedInstance;
static UIImagePickerController *_imagePicker = NULL;


+ (id)sharedInstance {
    
    if (_sharedInstance == nil)  {
        _sharedInstance = [[self alloc] init];
    }
    
    return _sharedInstance;
}

- (void) saveToCameraRoll:(NSString *)media {
    NSLog(@"saveToCameraRoll");
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];

#if UNITY_VERSION < 500
    [imageData release];
#endif
    
    UIImageWriteToSavedPhotosAlbum(image,
                                   self, // send the message to 'self' when calling the callback
                                   @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), // the selector to tell the method to call on completion
                                   NULL); // you generally won't need a contextInfo here
}

- (void)sendMessageToUnity:(NSString *)func Data:(NSString *)data
{
    UnitySendMessage("ImagePickerManager", [func UTF8String], [data UTF8String]);
}


- (void)thisImage:(UIImage *)image hasBeenSavedInPhotoAlbumWithError:(NSError *)error usingContextInfo:(void*)ctxInfo {
   
#if UNITY_VERSION < 500
    [image release];
    image=  nil;
#endif
    
    if (error) {
        NSLog(@"image not saved: %@", error.description);
        [self sendMessageToUnity:@"OnImageSaveFailed" Data:@""];
    } else {
        NSLog(@"image saved");
        [self sendMessageToUnity:@"OnImageSaveSuccess" Data:@""];
    }
    
    
}


-(void) PickImage:(int)source AllowsEditing:(bool)allowsEditing CropSize:(CGSize)size{
    
    UIImagePickerControllerSourceType SourceType;
    
    switch (source) {
        case 0:
            SourceType = UIImagePickerControllerSourceTypePhotoLibrary;
            break;
            
        case 1:
            SourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
            break;
            
        case 2:
            SourceType =  UIImagePickerControllerSourceTypeCamera;
            break;
            
        default:
            break;
    }
    
    _cropSize = size;
    
    if(SourceType == UIImagePickerControllerSourceTypeCamera) {
        [self GetImageFromCamera];
    } else {
        [self GetImage:SourceType AllowsEditing:allowsEditing];
    }
    
    
    
}


-(void) StartCameraImagePic {
    NSLog(@"StartCameraImagePic");
    [self GetImage:UIImagePickerControllerSourceTypeCamera AllowsEditing:false];
}

-(void) GetImageFromCamera {
    BOOL cameraAvailableFlag = [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
    if (cameraAvailableFlag) {
        [self performSelector:@selector(StartCameraImagePic) withObject:nil afterDelay:0.9];
    }
}


-(void) GetVideoPathFromAlbum {
    [self GetVideo];
}


- (void)GetVideo {
    UIViewController *vc =  UnityGetGLViewController();
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    picker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie,      nil];
    [vc presentViewController:picker animated:YES completion:nil];
#if UNITY_VERSION < 500
    [picker release];
#endif
   
}


-(void) GetImage: (UIImagePickerControllerSourceType )source AllowsEditing:(bool)allowsEditing{
    UIViewController *vc =  UnityGetGLViewController();
    
    if(_imagePicker == NULL) {
        _imagePicker = [[UIImagePickerController alloc] init];
        _imagePicker.delegate = self;
    }

    _allowsEditing = allowsEditing;
   // _imagePicker.allowsEditing = allowsEditing;
    _imagePicker.sourceType = source;
    
    
    [vc presentViewController:_imagePicker animated:YES completion:nil];
 
    
}


-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
//    [vc dismissViewControllerAnimated:YES completion:nil];
    
    
    // added video support
    NSString *mediaType = [info objectForKey: UIImagePickerControllerMediaType]; // get media type
    // if mediatype is video
    if (CFStringCompare ((__bridge CFStringRef) mediaType, kUTTypeMovie, 0) == kCFCompareEqualTo) {
        [vc dismissViewControllerAnimated:YES completion:nil];
        
        NSURL *videoUrl=(NSURL*)[info objectForKey:UIImagePickerControllerMediaURL];
        NSString *moviePath = [videoUrl path];
        [self sendMessageToUnity:@"OnVideoPickedEvent" Data:moviePath];
    } else{
        // it must be an image
        UIImage *photo = nil;
        if ([picker allowsEditing])
        {
            photo = [info objectForKey:UIImagePickerControllerEditedImage];
        }
        else
        {
            photo = [info objectForKey:UIImagePickerControllerOriginalImage];
        }
        NSString *encodedImage = @"";
        if (photo == nil) {
            NSLog(@"no photo");
        } else {
            // NSLog(@"MaxImageSize: %i", [self MaxImageSize]);
            //  NSLog(@"photo.size.width: %f", photo.size.width);
            
           photo = [photo fixOrientation];
            
//            NSData *imageData = nil;
//            if(UIImagePNGRepresentation(photo)) {
//                imageData = UIImagePNGRepresentation(photo);
//            } else {
//                imageData = UIImageJPEGRepresentation(photo, 1);
//            }
//            encodedImage = [imageData base64Encoding];
            
            if (_allowsEditing)
            {
                CropController *crop = [[CropController alloc] initWithImage:photo CropSize:_cropSize];
                crop.delegate = self;
                [picker pushViewController:crop animated:YES];
#if !__has_feature(objc_arc)
                [crop release];
#endif
            }
            else
            {
                [self passImage:photo];
            }
        }

        
//        [self sendMessageToUnity:@"OnImagePickedEvent" Data:encodedImage];
    }
    
}


- (void) passImage:(UIImage *)image
{
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    if (image)
    {
        // image = [image fixOrientation];
        NSString *encodedImage = @"";
        NSData *imageData = nil;
        if(UIImagePNGRepresentation(image)) {
            imageData = UIImagePNGRepresentation(image);
        } else {
            imageData = UIImageJPEGRepresentation(image, 1);
        }
        encodedImage = [imageData base64Encoding];
     
        [self sendMessageToUnity:@"OnImagePickedEvent" Data:encodedImage];
    }
    else
    {
        [self sendMessageToUnity:@"OnImagePickedEvent" Data:@""];
    }
}

+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 1.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    //[vc dismissModalViewControllerAnimated:YES];
    
    [self sendMessageToUnity:@"OnImagePickedEvent" Data:@""];
}

extern "C" {
    
    
    //--------------------------------------
    //  IOS Native Plugin Section
    //--------------------------------------
    
    
    void __saveToCameraRoll(char* encodedMedia) {
        NSString *media = nil;
        if (encodedMedia != NULL) {
            media = [NSString stringWithUTF8String: encodedMedia];
        } else {
            media = [NSString stringWithUTF8String: ""];
        }

        [[IOSImagePickerManager sharedInstance] saveToCameraRoll:media];
    }
    
    
    void __getVideoPathFromAlbum() {
        [[IOSImagePickerManager sharedInstance] GetVideoPathFromAlbum];
    }
    
    void __pickImage(int source, bool allowsEditing, int width, int height) {
        CGSize size = CGSizeMake(width, height);
        [[IOSImagePickerManager sharedInstance] PickImage:source AllowsEditing:allowsEditing CropSize:size];
    }


    int __screenHeight() {
        if (BELOW_IOS_8) {
            if (UnityCurrentOrientation() == landscapeLeft || UnityCurrentOrientation() == landscapeRight) {
                return UnityGetGLViewController().view.frame.size.width;
            } else {
                return UnityGetGLViewController().view.frame.size.height;
            }
        } else {
            return UnityGetGLViewController().view.frame.size.height;
        }
    }

    int __screenWidth() {
        if (BELOW_IOS_8) {
            if (UnityCurrentOrientation() == landscapeLeft || UnityCurrentOrientation() == landscapeRight) {
                return UnityGetGLViewController().view.frame.size.height;
            } else {
                return UnityGetGLViewController().view.frame.size.width;
            }
        } else {
            return UnityGetGLViewController().view.frame.size.width;
        }
    }
}


@end

#endif
