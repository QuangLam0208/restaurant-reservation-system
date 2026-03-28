package com.hcmute.reservation.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.io.File;

@Configuration
public class WebConfig implements WebMvcConfigurer {

    @Override
    public void addResourceHandlers(ResourceHandlerRegistry registry) {
        // Ánh xạ URL /web-customer/** vào thư mục vật lý web-customer của project
        // Chúng ta lấy đường dẫn tuyệt đối dựa trên thư mục hiện tại của project
        String projectRoot = System.getProperty("user.dir");
        // Nếu user.dir trỏ vào reservation-api, ta cần lên 2 cấp để ra root
        File rootDir = new File(projectRoot);
        if (rootDir.getName().equals("reservation-api")) {
            // Đang chạy trong folder con backend-api\reservation-api
             projectRoot = rootDir.getParentFile().getParentFile().getAbsolutePath();
        }
        
        String frontendPath = "file:" + projectRoot + "/web-customer/";
        
        registry.addResourceHandler("/web-customer/**")
                .addResourceLocations(frontendPath);
                
        // Cho phép truy cập cả các file assets, js trong đó
        registry.addResourceHandler("/assets/**")
                .addResourceLocations(frontendPath + "assets/");
        registry.addResourceHandler("/js/**")
                .addResourceLocations(frontendPath + "js/");
    }
}
