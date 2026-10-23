module.exports = {
  testEnvironment: 'jsdom',
  roots: ['<rootDir>/src'],
  testMatch: ['**/__tests__/**/*.test.ts?(x)'],
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.ts'],
  transform: {
    '^.+\\.tsx?$': ['ts-jest', { tsconfig: '<rootDir>/tsconfig.jest.json' }],
  },
  moduleNameMapper: {
    '\\.(css)$': 'identity-obj-proxy',
    '^(\\.{1,2}/)+apiConfig$': '<rootDir>/src/__tests__/mocks/apiConfig.ts',
    '^axios$': 'axios/dist/node/axios.cjs',
  },
};
