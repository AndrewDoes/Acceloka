/** @type {import('next').NextConfig} */
const nextConfig = {
  /* config options here */

  // In recent versions of Next.js, this property has moved 
  // from 'experimental' to the top-level config.
  allowedDevOrigins: [
    'http://localhost:3000',
    'http://127.0.0.1:3000',
    'http://192.168.56.1:3000'
  ],

  // Ensure we don't have issues with the backend URL during build/dev
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL || 'http://192.168.56.1:5225',
  }
};

export default nextConfig;