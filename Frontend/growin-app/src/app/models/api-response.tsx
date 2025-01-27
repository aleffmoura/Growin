interface ApiResponse {
    success: boolean;
    errors?: ApiError[];
  }
  
  interface ApiError {
    propertyName: string;
    errorMessage: string;
  }
  