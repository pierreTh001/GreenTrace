import { ApiClient } from './ApiClient'

export type SubscriptionPlan = { id: string; code: string; label?: string; isActive?: boolean }
export type Subscription = { id: string; planId: string; status: string; startedAt: string; endsAt?: string|null }

export const SubscriptionsService = {
  async plans(): Promise<SubscriptionPlan[]> {
    return await ApiClient.get<SubscriptionPlan[]>('/api/subscriptions/plans')
  },
  async me(): Promise<Subscription | null> {
    return await ApiClient.get<Subscription | null>('/api/subscriptions/me')
  },
  async subscribe(planId: string): Promise<Subscription> {
    return await ApiClient.post<Subscription>('/api/subscriptions/subscribe', { planId })
  },
  async cancel(): Promise<Subscription | null> {
    return await ApiClient.post<Subscription | null>('/api/subscriptions/cancel')
  }
}
